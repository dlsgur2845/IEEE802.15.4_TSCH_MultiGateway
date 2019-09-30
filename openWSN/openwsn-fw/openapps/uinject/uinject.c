#include "opendefs.h"
#include "uinject.h"
#include "openqueue.h"
#include "openserial.h"
#include "packetfunctions.h"
#include "scheduler.h"
#include "IEEE802154E.h"
#include "schedule.h"
#include "icmpv6rpl.h"
#include "idmanager.h"
#include "neighbors.h"
#include "sensors.h"
#include "adc_sensor.h"

//=========================== variables =======================================

uinject_vars_t uinject_vars;

static uint8_t uinject_payload[] = "uinject";
static const uint8_t uinject_dst_addr[]   = {
   0xbb, 0xbb, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
};

//=========================== prototypes ======================================

void uinject_timer_cb(opentimers_id_t id);
void uinject_task_cb(void);
// void ErrorHandling(char* message);

//=========================== public ==========================================

void uinject_init(void) {

    // clear local variables
    memset(&uinject_vars,0,sizeof(uinject_vars_t));

    // register at UDP stack
    uinject_vars.desc.port              = WKP_UDP_INJECT;
    uinject_vars.desc.callbackReceive   = &uinject_receive;
    uinject_vars.desc.callbackSendDone  = &uinject_sendDone;
    openudp_register(&uinject_vars.desc);

    uinject_vars.period = UINJECT_PERIOD_MS;
    // start periodic timer
    uinject_vars.timerId = opentimers_create(TIMER_GENERAL_PURPOSE, TASKPRIO_UDP);
    opentimers_scheduleIn(
        uinject_vars.timerId,
        UINJECT_PERIOD_MS,
        TIME_MS,
        TIMER_PERIODIC,
        uinject_timer_cb
    );

        adc_sensor_init();

}

void uinject_sendDone(OpenQueueEntry_t* msg, owerror_t error) {

    if (error==E_FAIL){
        openserial_printError(
            COMPONENT_UINJECT,
            ERR_UINJECT_PACKET_DROPPED,
            (errorparameter_t)uinject_vars.counter,
            (errorparameter_t)0
        );
    }

    // free the packet buffer entry
    openqueue_freePacketBuffer(msg);

    // allow send next uinject packet
    uinject_vars.busySendingUinject = FALSE;
}

// If Server transmit a packet, Mote will receive the packet
void uinject_receive(OpenQueueEntry_t* pkt) {

    openqueue_freePacketBuffer(pkt);
}

//=========================== private =========================================

void uinject_timer_cb(opentimers_id_t id){
    // calling the task directly as the timer_cb function is executed in
    // task mode by opentimer already
    uinject_task_cb();
}

void uinject_task_cb(void) {
    OpenQueueEntry_t*    pkt;
    uint8_t              asnArray[5];
    open_addr_t          parentNeighbor;
    bool                 foundNeighbor;

    // don't run if not synch
    if (ieee154e_isSynch() == FALSE) {
        return;
    }

    // don't run on dagroot
    if (idmanager_getIsDAGroot()) {
        opentimers_destroy(uinject_vars.timerId);
        return;
    }

    foundNeighbor = icmpv6rpl_getPreferredParentEui64(&parentNeighbor);
    if (foundNeighbor==FALSE) {
        return;
    }

    // if (schedule_hasManagedTxCellToNeighbor(&parentNeighbor) == FALSE) {
    //     return;
    // }

    if (uinject_vars.busySendingUinject==TRUE) {
        // don't continue if I'm still sending a previous uinject packet
        return;
    }

    // if you get here, send a packet

    // get a free packet buffer
    pkt = openqueue_getFreePacketBuffer(COMPONENT_UINJECT);
    if (pkt==NULL) {
        openserial_printError(
            COMPONENT_UINJECT,
            ERR_NO_FREE_PACKET_BUFFER,
            (errorparameter_t)0,
            (errorparameter_t)0
        );
        return;
    }

    pkt->owner                         = COMPONENT_UINJECT;
    pkt->creator                       = COMPONENT_UINJECT;
    pkt->l4_protocol                   = IANA_UDP;
    pkt->l4_destination_port           = WKP_UDP_INJECT;
    pkt->l4_sourcePortORicmpv6Type     = WKP_UDP_INJECT;
    pkt->l3_destinationAdd.type        = ADDR_128B;
    
    // Destination Address <== Root Mac Address
    memcpy(&pkt->l3_destinationAdd.addr_128b[0],uinject_dst_addr,16);

    // add payload
    // packetfunctions_reserveHeaderSize(pkt,sizeof(uinject_payload)-1);
    // memcpy(&pkt->payload[0],uinject_payload,sizeof(uinject_payload)-1);

    // get Neighbors(Gateway, Transmitter) Rssi and Address
    if(idmanager_getMyID(ADDR_64B)->addr_64b[7] == 0x33 || idmanager_getMyID(ADDR_64B)->addr_64b[7] == 0x66) {
        int8_t test;    // RSSI
        // Make space for RSSI and Address
        // Gateway, Transmitter Mote Address
        packetfunctions_reserveHeaderSize(pkt, sizeof(uint8_t)*neighbors_getNumNeighbors()*2);
        // Gateway, Transmitter Mote RSSI
        packetfunctions_reserveHeaderSize(pkt, sizeof(uint8_t)*neighbors_getNumNeighbors()*5);
        // Patient Mote Address, RSSI, BPM
        packetfunctions_reserveHeaderSize(pkt, sizeof(uint8_t)*7);
        for(int i = 0; i < neighbors_getNumNeighbors(); i++) {
            test = neighbors_getRssi(i);
            if(test>>7 & 0x1)
                test = ~test + 1;
            
            pkt->payload[i*2] = neighbors_getAddress(i)->addr_64b[7];
            pkt->payload[i*2+1] = test;
            pkt->payload[neighbors_getNumNeighbors()*2+i*5] = neighbors_getASN(i).byte4;
            pkt->payload[neighbors_getNumNeighbors()*2+i*5+1] = neighbors_getASN(i).bytes2and3 >> 8;
            pkt->payload[neighbors_getNumNeighbors()*2+i*5+2] = neighbors_getASN(i).bytes2and3 & 0xFF;
            pkt->payload[neighbors_getNumNeighbors()*2+i*5+3] = neighbors_getASN(i).bytes0and1 >> 8;
            pkt->payload[neighbors_getNumNeighbors()*2+i*5+4] = neighbors_getASN(i).bytes0and1 & 0xFF;
        }
        ieee154e_getAsn(asnArray);
        pkt->payload[neighbors_getNumNeighbors()*7] = idmanager_getMyID(ADDR_64B)->addr_64b[7];
        pkt->payload[neighbors_getNumNeighbors()*7+1] = asnArray[4];
        pkt->payload[neighbors_getNumNeighbors()*7+2] = asnArray[3];
        pkt->payload[neighbors_getNumNeighbors()*7+3] = asnArray[2];
        pkt->payload[neighbors_getNumNeighbors()*7+4] = asnArray[1];
        pkt->payload[neighbors_getNumNeighbors()*7+5] = asnArray[0];
        pkt->payload[neighbors_getNumNeighbors()*7+6] = getBeatPerMinute();
    }

    // packetfunctions_reserveHeaderSize(pkt,sizeof(uint16_t));
    // pkt->payload[1] = (uint8_t)((uinject_vars.counter & 0xff00)>>8);
    // pkt->payload[0] = (uint8_t)(uinject_vars.counter & 0x00ff);
    // uinject_vars.counter++;

    // packetfunctions_reserveHeaderSize(pkt,sizeof(asn_t));
    // ieee154e_getAsn(asnArray);
    // pkt->payload[0] = asnArray[0];
    // pkt->payload[1] = asnArray[1];
    // pkt->payload[2] = asnArray[2];
    // pkt->payload[3] = asnArray[3];
    // pkt->payload[4] = asnArray[4];

    if ((openudp_send(pkt))==E_FAIL) {
        openqueue_freePacketBuffer(pkt);
    } else {
        // set busySending to TRUE
        uinject_vars.busySendingUinject = TRUE;
    }
}
