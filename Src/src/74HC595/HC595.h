#ifndef HC595_h
#define HC595_h

#include <Arduino.h>

//74HC595
#define MRES 36
#define MOUT 35

#define MRCLK 39
#define MRLOCK 34
#define M1RED 27
#define M2RED 14
#define M3RED 12
#define M4RED 13

#define MBCLK 8
#define MBLOCK 7
#define M1BLU 9
#define M2BLU 10
#define M3BLU 11
#define M4BLU 6

class HC595
{
private:
public:
    HC595();
    void Reset();
    void SetOut(bool out);
    void SetRData(uint8_t *data1, uint8_t *data2, uint8_t *data3, uint8_t *data4, uint16_t size);
    void SetBData(uint8_t *data1, uint8_t *data2, uint8_t *data3, uint8_t *data4, uint16_t size);
};

extern class HC595 HC595;

#endif