#include <stdio.h>
#include <HC138.h>
#include "driver/gpio.h"
#include "sdkconfig.h"

uint8_t local;

void HC138Init()
{
    gpio_pad_select_gpio(A0);
    gpio_pad_select_gpio(A1);
    gpio_pad_select_gpio(A2);
    gpio_pad_select_gpio(A3);
    gpio_pad_select_gpio(AEN);

    gpio_set_direction(A0, GPIO_MODE_OUTPUT);
    gpio_set_direction(A1, GPIO_MODE_OUTPUT);
    gpio_set_direction(A2, GPIO_MODE_OUTPUT);
    gpio_set_direction(A3, GPIO_MODE_OUTPUT);
    gpio_set_direction(AEN, GPIO_MODE_OUTPUT);

    gpio_set_level(A0, 1);
    gpio_set_level(A1, 1);
    gpio_set_level(A2, 1);
    gpio_set_level(A3, 1);
    gpio_set_level(AEN, 1);

    local = 0;
}

void SetPin()
{
    gpio_set_level(A0, local & 0x01);
    gpio_set_level(A1, local & 0x02);
    gpio_set_level(A2, local & 0x04);
    gpio_set_level(A3, local & 0x08);
}

void SetEnable(uint8_t enable)
{
    gpio_set_level(AEN, enable);
}

void AddPos()
{
    if (local >= 15)
        local = 0;
    else
        local++;
    SetPin();
}

void ReSetPos()
{
    local = 0;
    SetPin();
}

void SetPos(uint8_t pos)
{
    if(pos > 15)
        return;
    local = pos;
    SetPin();
}