#ifndef UART_h
#define UART_h

#include <stdio.h>

#define TXD1 34
#define RXD1 35
#define BUF_SIZE 1024

extern uint8_t *UARTRData;
extern char *UARTTData;

void UartInit();
uint8_t Check(uint8_t *TempData);
void BuildPack(char *TempData);

#endif