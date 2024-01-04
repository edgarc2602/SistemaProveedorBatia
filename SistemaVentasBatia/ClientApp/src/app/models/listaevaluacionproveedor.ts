import { EvaluacionProveedor } from "./evaluacionproveedor";

export interface ListaEvaluacionProveedor {

    evaluacion: EvaluacionProveedor[];
    idEvaluacionProveedor: number;
    idProveedor: number;
    idStatus: number;
    fechaEvaluacion: string;
    numeroContrato: string
    promedio: number;
    textoPromedio: string;
    idUsuario: number;
}