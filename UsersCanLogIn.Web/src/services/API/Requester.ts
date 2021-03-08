import APIResponse, { APIErrorResponse, APISuccessResponse } from './types/APIResponses';
import APIResponseError, { NoAPIResponseError } from './util/APIResponseError';

export interface Requester {
    get(path: string, queryParams?: { [key: string]: string }): Promise<APISuccessResponse>;
    post(path: string, data?: object, queryParams?: { [key: string]: string }): Promise<APISuccessResponse>;
    put(path: string, data?: object, queryParams?: { [key: string]: string }): Promise<APISuccessResponse>;
    patch(path: string, data?: object, queryParams?: { [key: string]: string }): Promise<APISuccessResponse>;
    delete(path: string, queryParams?: { [key: string]: string }): Promise<APISuccessResponse>;
}

export class FetchRequester implements Requester {
    private static readonly ROUTE_PREFIX: string = '/api/v1/';

    public async get(path: string, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        return await this.requestWithoutBody('GET', path, queryParams);
    }

    public async post(path: string, data: object = {}, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        return await this.requestWithBody('POST', path, data, queryParams);
    }

    public async put(path: string, data: object = {}, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        return await this.requestWithBody('PUT', path, data, queryParams);
    }

    public async patch(path: string, data: object = {}, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        return await this.requestWithBody('PATCH', path, data, queryParams);
    }

    public async delete(path: string, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        return await this.requestWithoutBody('DELETE', path, queryParams);
    }

    public async requestWithoutBody(method: string, path: string, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        const url: string = this.getFetchUrl(path, queryParams);
        try {
            return this.tryReturnAPIResponse(await fetch(url, {
                method: method,
                credentials: 'same-origin',
            }));
        }
        catch (error) {
            if (!(error instanceof APIResponseError))
                throw new NoAPIResponseError();
            else
                throw error;
        }
    }

    private async requestWithBody(method: string, path: string, data: object = {}, queryParams: { [key: string]: string } = {}): Promise<APISuccessResponse> {
        const url: string = this.getFetchUrl(path, queryParams);
        try {
            return this.tryReturnAPIResponse(await fetch(url, {
                method: method,
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            }));
        }
        catch (error) {
            console.log(error);
            if (!(error instanceof APIResponseError))
                throw new NoAPIResponseError();
            else
                throw error;
        }
    }

    private getFetchUrl(path: string, queryParams: { [key: string]: string }): string {
        const pathTrimmed = path.startsWith('/') ? path.replace('/', '') : path;
        const url: string = `${FetchRequester.ROUTE_PREFIX}${pathTrimmed}`
        const queryParamsString: string = this.getQueryParamsString(queryParams);
        return queryParamsString ? `${url}?${queryParamsString}` : url;
    }

    private getQueryParamsString(queryParams: { [key: string]: string }): string {
        return new URLSearchParams(queryParams).toString();
    }

    private async tryReturnAPIResponse(response: Response): Promise<APISuccessResponse> {
        try {
            const responseBodyText = await response.text();
            const responseBody: APIResponse = responseBodyText ? JSON.parse(responseBodyText) : {};
            if (!response.ok)
                throw new APIResponseError(response.status, (responseBody as APIErrorResponse).errors);
            return { ...(responseBody as APISuccessResponse), statusCode: response.status };
        }
        catch (error) {
            if (!(error instanceof APIResponseError))
                throw new APIResponseError(response.status);
            else
                throw error;
        }
    }
}