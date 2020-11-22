#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "driver/uart.h"
#include "Uart.h"
#include "TaskUart.h"
#include "DataSave.h"

void TaskUart(void *data)
{
    BuildPack(UARTTData);
    printf("开始串口\n");
    for (;;)
    {
        int len = uart_read_bytes(UART_NUM_1, UARTRData, BUF_SIZE, 20 / portTICK_RATE_MS);
        if (len > 0)
        {
            if (Check(UARTRData))
            {
                printf("数据包错误\n");
                continue;
            }
            switch (UARTRData[5])
            {
            case 0x01:
                SetRun(UARTRData, 6);
                UARTTData[5] = 0x01;
                UARTTData[6] = 's';
                UARTTData[7] = 'e';
                UARTTData[8] = 't';
                uart_write_bytes(UART_NUM_1, UARTTData, 9);
                break;
            case 0x02:
                SetShow(UARTRData, 6);
                UARTTData[5] = 0x02;
                UARTTData[6] = 'o';
                UARTTData[7] = 'k';
                uart_write_bytes(UART_NUM_1, UARTTData, 8);
                break;
            }
        }
        vTaskDelay(20 / portTICK_PERIOD_MS);
    }
}