import { StatusCodes } from 'http-status-codes';

export default interface APIResponse {
    statusCode: StatusCodes;
}

export interface APISuccessResponse extends APIResponse {
    data: any;
}

export interface APIErrorResponse extends APIResponse {
    errors: APIError[];
}

export interface APIError {
    status: StatusCodes;
    title: string;
    detail: string;
}