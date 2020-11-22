#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "driver/uart.h"
#include "Uart.h"

uint8_t *UARTRData;
char *UARTTData;

uint8_t const ThisPack[5] = {0xaa, 0x53, 0xea, 0xda, 0x12};

void UartInit()
{
    uart_config_t uart_config = {
        .baud_rate = 115200,
        .data_bits = UART_DATA_8_BITS,
        .parity = UART_PARITY_DISABLE,
        .stop_bits = UART_STOP_BITS_1,
        .flow_ctrl = UART_HW_FLOWCTRL_DISABLE
    };
    uart_param_config(UART_NUM_1, &uart_config);
    uart_set_pin(UART_NUM_1, TXD1, RXD1, UART_PIN_NO_CHANGE, UART_PIN_NO_CHANGE);
    uart_driver_install(UART_NUM_1, BUF_SIZE * 2, BUF_SIZE * 2, 0, NULL, 0);

    UARTRData = (uint8_t *)malloc(BUF_SIZE);
    UARTTData = (char *)malloc(BUF_SIZE);
}

uint8_t Check(uint8_t *TempData)
{
    for (uint8_t i = 0; i < 5; i++)
    {
        if (TempData[i] != ThisPack[i])
            return 1;
        else
            TempData[i] = 0;
    }
    return 0;
}

void BuildPack(char *TempData)
{
    for (uint8_t i = 0; i < 5; i++)
    {
        TempData[i] = ThisPack[i];
    }
}