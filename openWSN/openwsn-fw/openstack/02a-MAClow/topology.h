#ifndef __TOPOLOGY_H
#define __TOPOLOGY_H

/**
\addtogroup MAClow
\{
\addtogroup topology
\{
*/

#include "opendefs.h"
#include "IEEE802154.h"

//=========================== define ==========================================

// #define NCHILDREN 1

// static const uint8_t EUI64_ROOT[] = {
//     0x00, 0x12, 0x4b, 0x00, 0x04, 0x22, 0xc4, 0x43
// };
// static const uint8_t EUI64_CHILD_01[] = {
//     0x00, 0x12, 0x4b, 0x00, 0x04, 0x22, 0x5e, 0xad
// };
// // static const uint8_t EUI64_CHILD_02[] = {
// //     0x00, 0x12, 0x4b, 0x00, 0x04, 0x30, 0xc4, 0x3b
// // };
// // static const uint8_t EUI64_CHILD_03[] = {
// //     0x00, 0x12, 0x4b, 0x00, 0x04, 0x30, 0x5e, 0xad
// // };
// // static const uint8_t EUI64_CHILD_04[] = {
// //     0x00, 0x12, 0x4b, 0x00, 0x04, 0x30, 0x5e, 0xe1
// // };
// // static const uint8_t EUI64_CHILD_05[] = {
// //     0x00, 0x12, 0x4b, 0x00, 0x04, 0x30, 0xc4, 0xc6
// // };
// static const uint8_t *EUI64_CHILDREN[] = {
//     EUI64_CHILD_01,
// //     EUI64_CHILD_02,
// //     EUI64_CHILD_03,
// //     EUI64_CHILD_04,
// //     EUI64_CHILD_05,
// };

//=========================== typedef =========================================

//=========================== variables =======================================

//=========================== prototypes ======================================

//=========================== prototypes ======================================

bool topology_isAcceptablePacket(ieee802154_header_iht *ieee802514_header);

/**
\}
\}
*/

#endif
