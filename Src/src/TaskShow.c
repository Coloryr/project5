#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "sdkconfig.h"
#include "esp_task_wdt.h"
#include "TaskShow.h"
#include "DataSave.h"
#include "HC138.h"
#include "HC595.h"

void TaskShow(void *data)
{
    uint8_t y;
    printf("开始显示\n");
    SetEnable(0);
    for (;;)
    {
        if (CanRun)
        {
            //扫描一次
            ReSetPos();
            for (y = 0; y < Height; y++)
            {
                if (Height > 16)
                {
                    SetDataB(REDData[Height], BULData[Height], REDData[Height + 16], BULData[Height + 16], Width);
                }
                else
                {
                    SetDataA(REDData[Height], BULData[Height], Width);
                }

                AddPos();
                UnLock();
                // SetEnable(1);
                // esp_task_wdt_reset();
                vTaskDelay(4 / portTICK_PERIOD_MS);
            }
        }
        else
        {
            SetEnable(1);
        }
    }
}