#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "DataSave.h"

uint8_t Height;
uint8_t Width;
uint8_t CanRun;

uint8_t **REDData;
uint8_t **BULData;

void DataInit()
{
    Height = 16;
    Width = 4;
    CanRun = 1;

    REDData = NULL;
    BULData = NULL;

    InitShow();
}

void FreeShow()
{
    printf("释放完毕\n");
    if (REDData != NULL)
    {
        for (uint8_t i = 0; i < Height; ++i)
        {
            free(REDData[i]);
        }
        free(REDData);
    }
    if (BULData != NULL)
    {
        for (uint8_t i = 0; i < Height; ++i)
        {
            free(BULData[i]);
        }
        free(BULData);
    }
}

void InitShow()
{
    printf("设置屏幕参数%d,%d\n", Height, Width);

    REDData = (uint8_t **)malloc(Height * sizeof(uint8_t *));
    BULData = (uint8_t **)malloc(Height * sizeof(uint8_t *));

    for (uint8_t i = 0; i < Height; ++i)
    {
        REDData[i] = (uint8_t *)malloc(Width * sizeof(uint8_t));
        BULData[i] = (uint8_t *)malloc(Width * sizeof(uint8_t));
    }
}

void SetRun(uint8_t *data, uint8_t start)
{
    printf("正在设置屏幕\n");
    CanRun = 0;
    vTaskDelay(50 / portTICK_PERIOD_MS);

    FreeShow();

    Height = data[start];
    Width = data[start + 1] / 8;

    InitShow();

    CanRun = 1;
}

void SetShow(uint8_t *data, uint8_t start)
{
    uint8_t count = start;
    uint8_t next = start + Height * Width;
    for (uint8_t i = 0; i < Height; i++)
    {
        for (uint8_t j = 0; j < Width; j++)
        {
            REDData[i][j] = data[count++];
            BULData[i][j] = data[next++];
        }
    }
}
