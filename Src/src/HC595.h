#ifndef HC595_h
#define HC595_h

#include <stdio.h>

//74HC595
#define MLOCK 19
#define MCLK 5
#define MRES 17
#define M1RED 22
#define M1BLU 23
#define M2RED 21
#define M2BLU 18

void HC595Init();
void Reset();
void UnLock();
void SetDataA(uint8_t *data1, uint8_t *data2, uint16_t size);
void SetDataB(uint8_t *data1, uint8_t *data2,uint8_t *data3, uint8_t *data4, uint16_t size);

#endif