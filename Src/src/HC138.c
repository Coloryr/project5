#include <stdio.h>
#include <HC138.h>

void Init()
{
    // pinMode(A0, OUTPUT);
    // pinMode(A1, OUTPUT);
    // pinMode(A2, OUTPUT);
    // pinMode(AEN, OUTPUT);

    local = 0;

    // digitalWrite(AEN, HIGH);
    // digitalWrite(A0, HIGH);
    // digitalWrite(A1, HIGH);
    // digitalWrite(A2, HIGH);
}

void SetPin()
{
    // digitalWrite(A0, local & 0x01);
    // digitalWrite(A1, local & 0x02);
    // digitalWrite(A2, local & 0x04);
}

void SetEnable(bool enable)
{
    // digitalWrite(AEN, enable ? LOW : HIGH);
}

void AddPos()
{
    if (local >= 7)
        local = 0;
    else
        local++;
    SetPin();
}

void SetPos(uint8_t pos)
{
    if (pos > 7)
        return;
    local = pos;
    SetPin();
}