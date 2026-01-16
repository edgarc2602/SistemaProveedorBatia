export interface Recepcion {
    idRecepcion: number;
    idOrdenCompra: number;
    idKardex: number;
    fecha: string;
    importe: number;
    iva: number;
    total: number;
    factura: string;
    interface: number;
    id_usuario: number;
    fechaFactura: string;
    uuid: string;
    ret_isr: number;
    ret_iva: number;
    ieps: number;
    descuento: number;
}