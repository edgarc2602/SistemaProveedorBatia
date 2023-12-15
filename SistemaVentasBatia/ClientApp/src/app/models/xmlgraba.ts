export interface XMLGraba {
    factura: string;
    idCliente: number;
    idOrden: number;
    idPersonal: number;
    fechaFactura: string;
    dias: number;
    subTotal: number;
    iva: number;
    total: number;
    pdfName: string;
    xmlName: string;
}