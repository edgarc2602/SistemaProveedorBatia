export interface EstadoDeCuenta {
    nombre: string;
    factura: string;
    total: number;
    pago: number;
    saldo: number;
    ffactura: Date;
    fvencimiento: Date;
    diasCredito: number;
    diasVencido: number;
    corriente: number;
    mes1: number;
    mes2: number;
    mes3: number;
    mes4: number;
}