export interface Requisicion {
    idRequisicion: number;
    idProveedor: number;
    comprador: string;
    comentarios: string;
    fechaAlta: string;
    idEstatus: number;
    estatus: string;
    iva: number;
    ivaPorcentaje: number;
    subtotal: number;
    total: number;
    ivaNuevo: number;
    subtotalNuevo: number;
    totalNuevo: number;
    idOrdenCompra: number;
}