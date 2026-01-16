export interface OrdenCompra {
    idOrden: number;
    tipo: string;
    estatus: string;
    fechaAlta: string;
    empresa: string;
    proveedor: string;
    idCliente: number;
    cliente: string;
    elabora: string;
    observacion: string;
    inventario: number;
    facturado: number;
    idCredito: number;
    credito: string;
    diasCredito: number;
    idAlmacen: number;
    loading: boolean;
    iva: number;
    subTotal: number;
    total: number;
}