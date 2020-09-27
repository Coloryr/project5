#ifndef HC138_h
#define HC138_h

#include <Arduino.h>

//74HC138
#define A0 32
#define A1 33
#define A2 25
#define AEN 26
class HC138
{
private:
    uint8_t local;
    void SetPin();
public:
    HC138();
    void SetEnable(bool enable);
    void AddPos();
    void SetPos(uint8_t pos);
};

extern class HC138 HC138;

#endif