#include <Arduino.h>
#include <74HC138/HC138.h>

class HC138 HC138;

HC138::HC138()
{
}

void HC138::Init()
{
    pinMode(A0, OUTPUT);
    pinMode(A1, OUTPUT);
    pinMode(A2, OUTPUT);
    pinMode(AEN, OUTPUT);

    local = 0;

    digitalWrite(AEN, HIGH);
    digitalWrite(A0, HIGH);
    digitalWrite(A1, HIGH);
    digitalWrite(A2, HIGH);
}

void HC138::SetPin()
{
    digitalWrite(A0, local & 0x01);
    digitalWrite(A1, local & 0x02);
    digitalWrite(A2, local & 0x04);
}

void HC138::SetEnable(bool enable)
{
    digitalWrite(AEN, enable ? LOW : HIGH);
}

void HC138::AddPos()
{
    if (local >= 7)
        local = 0;
    else
        local++;
    SetPin();
}

void HC138::SetPos(uint8_t pos)
{
    if (pos > 7)
        return;
    local = pos;
    SetPin();
}