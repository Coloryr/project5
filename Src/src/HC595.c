#include <stdio.h>
#include <HC595.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "sdkconfig.h"

void HC595Init()
{
    gpio_pad_select_gpio(MRES);
    gpio_pad_select_gpio(MLOCK);
    gpio_pad_select_gpio(MCLK);
    gpio_pad_select_gpio(M1RED);
    gpio_pad_select_gpio(M1BLU);
    gpio_pad_select_gpio(M2RED);
    gpio_pad_select_gpio(M2BLU);

    gpio_set_direction(MRES, GPIO_MODE_OUTPUT);
    gpio_set_direction(MLOCK, GPIO_MODE_OUTPUT);
    gpio_set_direction(MCLK, GPIO_MODE_OUTPUT);
    gpio_set_direction(M1RED, GPIO_MODE_OUTPUT);
    gpio_set_direction(M1BLU, GPIO_MODE_OUTPUT);
    gpio_set_direction(M2RED, GPIO_MODE_OUTPUT);
    gpio_set_direction(M2BLU, GPIO_MODE_OUTPUT);

    gpio_set_level(MRES, 1);
    gpio_set_level(MLOCK, 0);
    gpio_set_level(MCLK, 0);
    gpio_set_level(M1RED, 1);
    gpio_set_level(M1BLU, 1);
    gpio_set_level(M2RED, 1);
    gpio_set_level(M2BLU, 1);

    Reset();
}

void Reset()
{
    gpio_set_level(MRES, 0);
    vTaskDelay(10 / portTICK_PERIOD_MS);
    gpio_set_level(MRES, 1);
}

void UnLock()
{
    gpio_set_level(MLOCK, 0);
    ets_delay_us(10);
    gpio_set_level(MLOCK, 1);
}

void SetDataA(uint8_t *data1, uint8_t *data2, uint8_t size)
{
    uint8_t i;
    uint8_t local;
    uint8_t data_1;
    uint8_t data_2;

    for (local = 0; local < size; local++)
    {
        data_1 = data1[local];
        data_2 = data2[local];
        for (i = 0; i < 8; i++)
        {
            gpio_set_level(MCLK, 0);
            // gpio_set_level(M1BLU, 0);
            // gpio_set_level(M1RED, 1);
            gpio_set_level(M1RED, data_1 & 0x80 ? 1 : 0);
            gpio_set_level(M1BLU, data_2 & 0x80 ? 1 : 0);

            data_1 <<= 1;
            data_2 <<= 1;
            gpio_set_level(MCLK, 1);
        }
    }
}

void SetDataB(uint8_t *data1, uint8_t *data2, uint8_t *data3, uint8_t *data4, uint8_t size)
{
    uint8_t i;
    uint8_t local;
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
            gpio_set_level(MCLK, 0);
            if (data_1 & 0x80)
            {
                gpio_set_level(M1RED, 1);
            }
            else
            {
                gpio_set_level(M1RED, 0);
            }

            if (data_2 & 0x80)
            {
                gpio_set_level(M1BLU, 1);
            }
            else
            {
                gpio_set_level(M1BLU, 0);
            }

            if (data_3 & 0x80)
            {
                gpio_set_level(M2RED, 1);
            }
            else
            {
                gpio_set_level(M2RED, 0);
            }

            if (data_4 & 0x80)
            {
                gpio_set_level(M2BLU, 1);
            }
            else
            {
                gpio_set_level(M2BLU, 0);
            }

            data_1 <<= 1;
            data_2 <<= 1;
            data_3 <<= 1;
            data_4 <<= 1;

            gpio_set_level(MCLK, 1);
        }
    }
}