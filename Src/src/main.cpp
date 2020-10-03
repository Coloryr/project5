#include <Arduino.h>
#include <pin.h>
#include <74HC138/HC138.h>
#include <74HC595/HC595.h>
#include <W25Q64/FLASH.h>
#include <W25Q64/W25Q64.h>
#include "esp_spi_flash.h"

#define SPIFFS_BASE_ADDR 0x291000 // SPIFFS領域のベースアドレス
uint32_t chip_size = 0;
uint8_t w_buf[SPI_FLASH_SEC_SIZE];
uint8_t r_buf[SPI_FLASH_SEC_SIZE];

//任务A
void Taska(void *data)
{
    //循环闪烁
    for (;;)
    {
        //写IO口为高
        digitalWrite(LED, HIGH);
        //延迟
        delay(300);
        //写IO口为低
        digitalWrite(LED, LOW);
        //延迟
        delay(300);

        W25Q64.W25Q64_readUniqieID();

        Serial.print("ID: ");
        Serial.println(W25Q64.W25QXX_TYPE, HEX);
    }
}

void setup()
{
    Serial.begin(115200);
    HC138.Init();
    HC595.Init();
    W25Q64.W25Q64_begin();
    W25Q64.W25Q64_readUniqieID();

   
    w_buf[0] = 0x65;

    Serial.printf("Write to SPI flash = %x\n\n", w_buf[0]);
    spi_flash_write(SPIFFS_BASE_ADDR, w_buf, 1);

     spi_flash_read(SPIFFS_BASE_ADDR, r_buf, SPI_FLASH_SEC_SIZE);
    Serial.printf("Read to SPI flash = %x\n\n", r_buf[0]);

    Serial.print("ID: ");
    Serial.println(W25Q64.W25QXX_TYPE, HEX);

    //设置引脚模式
    pinMode(LED, OUTPUT);
    //创建任务
    xTaskCreate(
        Taska,   //任务
        "TaskA", //任务名字
        8192,    //堆大小，太小会溢出
        NULL,    //参数
        2,       //优先级
        NULL     //控制指针
    );
}

void loop()
{
}