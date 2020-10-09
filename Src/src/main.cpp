#include <Arduino.h>
#include <pin.h>
#include <74HC138/HC138.h>
#include <74HC595/HC595.h>
#include <W25Q64/FLASH.h>
#include <W25Q64/W25Q64.h>
#include "esp_spi_flash.h"
#include "SD.h"
#include "SPI.h"

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
    // W25Q64.W25Q64_begin();
    // W25Q64.W25Q64_readUniqieID();

    SPIClass SPI(VSPI);

    //挂载文件系统
    if (!SD.begin(5, SPI))
    {
        Serial.println("存储卡挂载失败");
        return;
    }
    uint8_t cardType = SD.cardType();

    if (cardType == CARD_NONE)
    {
        Serial.println("未连接存储卡");
        return;
    }
    else if (cardType == CARD_MMC)
    {
        Serial.println("挂载了MMC卡");
    }
    else if (cardType == CARD_SD)
    {
        Serial.println("挂载了SDSC卡");
    }
    else if (cardType == CARD_SDHC)
    {
        Serial.println("挂载了SDHC卡");
    }
    else
    {
        Serial.println("挂载了未知存储卡");
    }

    //打开/建立 并写入数据
    File file = SD.open("/test.txt", FILE_WRITE);
    if (file)
    {
        Serial.println("打开/建立 根目录下 test.txt 文件！");
    }

    char data[] = "hello world\r\n";
    file.write((uint8_t *)data, strlen(data));
    file.close();

    //重命名文件
    if (SD.rename("/test.txt", "/retest.txt"))
    {
        Serial.println("test.txt 重命名为 retest.txt ！");
    }

    //读取文件数据
    file = SD.open("/retest.txt", FILE_READ);
    if (file)
    {
        Serial.print("文件内容是：");
        while (file.available())
        {
            Serial.print((char)file.read());
        }
    }

    //打印存储卡信息
    Serial.printf("存储卡总大小是： %lluMB \n", SD.cardSize() / (1024 * 1024)); // "/ (1024 * 1024)"可以换成">> 20"
    Serial.printf("文件系统总大小是： %lluB \n", SD.totalBytes());
    Serial.printf("文件系统已用大小是： %lluB \n", SD.usedBytes());

    // w_buf[0] = 0x65;

    // Serial.printf("Write to SPI flash = %x\n\n", w_buf[0]);
    // spi_flash_write(SPIFFS_BASE_ADDR, w_buf, 1);

    //  spi_flash_read(SPIFFS_BASE_ADDR, r_buf, SPI_FLASH_SEC_SIZE);
    // Serial.printf("Read to SPI flash = %x\n\n", r_buf[0]);

    // Serial.print("ID: ");
    // Serial.println(W25Q64.W25QXX_TYPE, HEX);

    // //设置引脚模式
    // pinMode(LED, OUTPUT);
    // //创建任务
    // xTaskCreate(
    //     Taska,   //任务
    //     "TaskA", //任务名字
    //     8192,    //堆大小，太小会溢出
    //     NULL,    //参数
    //     2,       //优先级
    //     NULL     //控制指针
    // );
}

void loop()
{
}