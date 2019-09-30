/**
    \brief Declaration of the OpenMote-CC2538 ADC temperature sensor driver.
    \author Nicola Accettura <nicola.accettura@eecs.berkeley.edu>, March 2015.
*/

#ifndef __ADC_SENSOR_H__
#define __ADC_SENSOR_H__

#include "board_info.h"
#include "opentimers.h"

//=========================== define ==========================================

//=========================== typedef =========================================

//=========================== module variables ================================

//=========================== prototypes ======================================


//================== pulse sensor ==================
#define UNKNOWN 0
#define NEGATIVE 1
#define POSITIVE 2

uint8_t pulse_interval;
uint8_t last_sign;

uint16_t Rate[20];
uint16_t global_time;
uint16_t last_pulse;
uint16_t count;
uint16_t IBI;

uint16_t adc_sens_read_pulse(void);
uint32_t adc_sens_read_pulse_finger(void);
uint8_t getBeatPerMinute(void);

void pulse_printError(opentimers_id_t id);
void adc_sensor_init(void);
void adc_sens_print_pulse(opentimers_id_t id);
void timer_ms(opentimers_id_t id);


//=================================================

uint16_t adc_sens_read_temperature(void);
float adc_sens_convert_temperature(uint16_t cputemp);

#endif // __ADC_SENSOR_H__