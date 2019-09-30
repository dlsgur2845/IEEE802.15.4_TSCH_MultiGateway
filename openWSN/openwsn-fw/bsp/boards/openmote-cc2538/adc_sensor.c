/**
   \brief Definition of the OpenMote-CC2538 ADC temperature sensor driver.
   \author Nicola Accettura <nicola.accettura@eecs.berkeley.edu>, March 2015.
*/

#include <headers/hw_cctest.h>
#include <headers/hw_rfcore_xreg.h>

#include <source/adc.h>

#include "adc_sensor.h"
#include "openserial.h"
#include "opentimers.h"
#include "scheduler.h"

//=========================== defines =========================================

#define CONST 0.58134 //(VREF / 2047) = (1190 / 2047), VREF from Datasheet
#define OFFSET_DATASHEET_25C 827 // 1422*CONST, from Datasheet
#define TEMP_COEFF (CONST * 4.2) // From Datasheet
#define OFFSET_0C (OFFSET_DATASHEET_25C - (25 * TEMP_COEFF))

//=========================== variables =======================================

//=========================== prototype =======================================

//=========================== public ==========================================

/**
   \brief Initialize the sensor
*/
void adc_sensor_init(void) {
    HWREG(CCTEST_TR0) |= CCTEST_TR0_ADCTM;
    HWREG(RFCORE_XREG_ATEST) = 0x01;
    SOCADCSingleConfigure(SOCADC_12_BIT, SOCADC_REF_INTERNAL);
    adc_sens_read_temperature();
   
    // Initialize variables
    last_pulse=0;
    last_sign=0;
    pulse_interval=0;
    global_time=0;
    count=0;
    IBI=0;

    for(uint8_t i=0;i<20;i++){
       Rate[i] = 0;
    }

    // Timer setting
    uint8_t timer_ms_id = opentimers_create(TIMER_GENERAL_PURPOSE, TASKPRIO_MAX);
    opentimers_scheduleIn(
       timer_ms_id,
       1,
       0,
       0,
       timer_ms
       );

    uint8_t timer_pulse_id = opentimers_create(TIMER_GENERAL_PURPOSE, 7);
    opentimers_scheduleIn(
       timer_pulse_id,
       50,
       0,
       0,
       adc_sens_print_pulse
       );
}


/**
   \brief Read rough data from sensor
   \param[out] ui16Dummy rough data.
*/
uint16_t adc_sens_read_temperature(void) {
   uint16_t ui16Dummy;

   SOCADCSingleStart(SOCADC_TEMP_SENS);
   while(!SOCADCEndOfCOnversionGet());
   ui16Dummy = SOCADCDataGet() >> SOCADC_12_BIT_RSHIFT;
   return ui16Dummy;
}

/**
   \brief Convert rough data to human understandable
   \param[in] cputemp rough data.
   \param[out] the number of registered OpenSensors.
*/
float adc_sens_convert_temperature(uint16_t cputemp) {
   float dOutputVoltage;

   dOutputVoltage = cputemp * CONST;
   dOutputVoltage = ((dOutputVoltage - OFFSET_0C) / TEMP_COEFF);
   return dOutputVoltage;
}

/**
   \brief Read rough data from sensor
   \param[out] ui16Dummy rough data.
*/

uint16_t adc_sens_read_pulse(void) {
   uint16_t ui16Dummy;

   SOCADCSingleStart(SOCADC_AIN2);
   while(!SOCADCEndOfCOnversionGet());
   ui16Dummy = SOCADCDataGet() >> SOCADC_12_BIT_RSHIFT;

   return ui16Dummy;
}

uint32_t adc_sens_read_pulse_finger(void){
   uint32_t ui16Dummy;

   SOCADCSingleStart(SOCADC_AIN4);
   while(!SOCADCEndOfCOnversionGet());
   ui16Dummy = SOCADCDataGet_32();

   if(ui16Dummy < 1000) return 0;

   return ui16Dummy;
}

void adc_sens_print_pulse(opentimers_id_t id){

   uint16_t cur_pulse = adc_sens_read_pulse_finger();
   if(cur_pulse==0) return;
   uint8_t sign=POSITIVE;
   uint8_t time_offset=0;

   if(last_pulse){
      int16_t slope = cur_pulse - last_pulse;

      if(slope<0){ sign=NEGATIVE; }
      
      if(!last_sign){
         last_sign=sign;
         return; 
      }

      if(last_sign==NEGATIVE || slope==0){
         if(sign==POSITIVE){
            global_time=0;
         }
      }
      else if(last_sign==POSITIVE || slope==0){
         if(sign==NEGATIVE){
            time_offset = global_time * 3;

            Rate[count] = time_offset;
       count = (count+1)%20;

    }
      }
   }
   last_pulse = cur_pulse;
   last_sign = sign;

}

void timer_ms(opentimers_id_t id){
   global_time=(global_time+1)%1501;
}

uint8_t getBeatPerMinute(){
   uint8_t BPM;
   IBI=0;
   for(uint8_t i=0;i<20;i++){
           IBI+=Rate[i];
   }
   IBI /= 20;
   BPM = 6000 / IBI;

   return BPM;
}

void pulse_printError(opentimers_id_t id){
   uint8_t BPM = getBeatPerMinute();
        openserial_printError(
           COMPONENT_TEST,
           ERR_UINJECT_TEST,
           (errorparameter_t)BPM,
           (errorparameter_t)202
              );
}


//=========================== private =========================================