export interface DetalleOrdenCompra {
    idOrden: number;
    idRequisicion: number;
    idProveedor: number;
    idCliente: number;
    proveedor: string;
    empresa: string;
    cliente: string;
    subTotal: number;
    iva: number;
    total: number;
    status: number;
    dias: number;
}