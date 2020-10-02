#include <Arduino.h>
#include <74HC595/HC595.h>

class HC595 HC595;

HC595::HC595()
{
}

void HC595::Init()
{
    pinMode(MRES, OUTPUT);
    pinMode(MRCLK, OUTPUT);
    pinMode(MRLOCK, OUTPUT);
    pinMode(MBCLK, OUTPUT);
    pinMode(MBLOCK, OUTPUT);
    pinMode(MOUT, OUTPUT);
    pinMode(M1RED, OUTPUT);
    pinMode(M2RED, OUTPUT);
    pinMode(M3RED, OUTPUT);
    pinMode(M4RED, OUTPUT);
    pinMode(M1BLU, OUTPUT);
    pinMode(M2BLU, OUTPUT);
    pinMode(M3BLU, OUTPUT);
    pinMode(M4BLU, OUTPUT);

    digitalWrite(MRES, LOW);
    digitalWrite(MBCLK, LOW);
    digitalWrite(MRCLK, LOW);
    delay(10);
    digitalWrite(MRES, HIGH);
}

void HC595::Reset()
{
    digitalWrite(MRES, LOW);
    delay(10);
    digitalWrite(MRES, HIGH);
}

void HC595::SetOut(bool out)
{
    digitalWrite(MOUT, out ? LOW : HIGH);
}

void HC595::SetRData(uint8_t *data1, uint8_t *data2, uint8_t *data3, uint8_t *data4, uint16_t size)
{
    uint8_t i;
    uint16_t local;
    uint8_t data_1;
    uint8_t data_2;
    uint8_t data_3;
    uint8_t data_4;

    for (local = 0; local < size; local++)
    {
        data_1 = data1[local];
        data_2 = data2[local];
        data_3 = data3[local];
        data_4 = data4[local];
        for (i = 0; i < 8; i++)
        {
            digitalWrite(M1RED, data_1);
            digitalWrite(M2RED, data_2);
            digitalWrite(M3RED, data_3);
            digitalWrite(M4RED, data_4);
            data_1 <<= 1;
            data_2 <<= 1;
            data_3 <<= 1;
            data_4 <<= 1;
            digitalWrite(MRCLK, LOW);
            delayMicroseconds(10);
            digitalWrite(MRCLK, HIGH);
        }

        digitalWrite(MRLOCK, HIGH);
        delayMicroseconds(10);
        digitalWrite(MRLOCK, LOW);
    }
}

void HC595::SetBData(uint8_t *data1, uint8_t *data2, uint8_t *data3, uint8_t *data4, uint16_t size)
{
    uint8_t i;
    uint16_t local;
    uint8_t data_1;
    uint8_t data_2;
    uint8_t data_3;
    uint8_t data_4;

    for (local = 0; local < size; local++)
    {
        data_1 = data1[local];
        data_2 = data2[local];
        data_3 = data3[local];
        data_4 = data4[local];
        for (i = 0; i < 8; i++)
        {
            digitalWrite(M1BLU, data_1);
            digitalWrite(M2BLU, data_2);
            digitalWrite(M3BLU, data_3);
            digitalWrite(M4BLU, data_4);
            data_1 <<= 1;
            data_2 <<= 1;
            data_3 <<= 1;
            data_4 <<= 1;
            digitalWrite(MBCLK, LOW);
            delayMicroseconds(10);
            digitalWrite(MBCLK, HIGH);
        }

        digitalWrite(MBLOCK, HIGH);
        delayMicroseconds(10);
        digitalWrite(MBLOCK, LOW);
    }
}
