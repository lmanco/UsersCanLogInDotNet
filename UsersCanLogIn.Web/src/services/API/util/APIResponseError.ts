import { APIError } from '../types/APIResponses';
import { StatusCodes } from 'http-status-codes';

export default class APIResponseError extends Error {
    private static readonly MESSAGE = 'The server returned an error response';
    private static readonly UNEXPECTED_ERROR_MESSAGE = 'An unexpected error has occurred.';

    public responseStatusCode: StatusCodes;
    public apiErrors: APIError[];
    public detailMessages: string[];

    public constructor(responseStatusCode: StatusCodes, errors: APIError[] = [], message: string = APIResponseError.MESSAGE) {
        super(message);
        this.responseStatusCode = responseStatusCode;
        this.apiErrors = errors;
        this.detailMessages = errors.length ?
            errors.map(error => error.detail) : [APIResponseError.UNEXPECTED_ERROR_MESSAGE]
    }

    public RewriteErrorDetail(errorTitle: string, newErrorDetail: string, append: boolean = false): void {
        const errorTitleLower = errorTitle.toLowerCase();
        this.apiErrors.filter(error => error.title.toLowerCase() === errorTitleLower).forEach(error => {
            error.detail = append ? `${error.detail}${newErrorDetail}` : newErrorDetail;
        });
        this.detailMessages = this.apiErrors.length ?
            this.apiErrors.map(error => error.detail) : [APIResponseError.UNEXPECTED_ERROR_MESSAGE]
    }
}

export class NoAPIResponseError extends APIResponseError {
    private static readonly NO_RESPONSE_MESSAGE = 'The server did not respond';
    private static readonly API_ERROR_TITLE = 'No Response';
    private static readonly API_ERROR_DETAIL = 'The remote server did not respond. Please try again.';

    public constructor() {
        super(0, [{
            status: 0,
            title: NoAPIResponseError.API_ERROR_TITLE,
            detail: NoAPIResponseError.API_ERROR_DETAIL
        }], NoAPIResponseError.NO_RESPONSE_MESSAGE);
    }
}