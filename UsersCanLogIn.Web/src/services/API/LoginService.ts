import { Requester } from '@/services/API/Requester';

export interface LoginService {
    login(username: string, password: string): Promise<void>;
    logout(): Promise<void>;
    resendVerificationEmail(email: string): Promise<void>;
    sendPasswordResetEmail(email: string): Promise<void>;
    checkPasswordResetToken(token: string): Promise<void>;
}

class Constants {
    public static readonly LOGIN_PATH: string = 'login';
    public static readonly LOGOUT_PATH: string = 'logout';
    public static readonly VERIFICATION_TOKEN_PATH: string = 'verification-tokens';
    public static readonly PASSWORD_RESET_TOKEN_PATH: string = 'password-reset-tokens';
}

export default class RequesterLoginService implements LoginService {
    private requester: Requester;
    private siteUrlOverride?: string;

    public constructor(requester: Requester, siteUrlOverride?: string) {
        this.requester = requester;
        this.siteUrlOverride = siteUrlOverride;
    }

    public async login(username: string, password: string): Promise<void> {
        const requestBody: object = this.isEmail(username) ?
            { email: username } : { username: username };
        await this.requester.post(Constants.LOGIN_PATH, {
            ...requestBody,
            password: password
        });
    }

    public async logout(): Promise<void> {
        await this.requester.post(Constants.LOGOUT_PATH);
    }

    public async resendVerificationEmail(username: string): Promise<void> {
        const requestBody: object = this.isEmail(username) ?
            { email: username } : { username: username };
        await this.requester.post(Constants.VERIFICATION_TOKEN_PATH, {
            ...requestBody,
            siteUrlOverride: this.siteUrlOverride
        });
    }

    public async sendPasswordResetEmail(username: string): Promise<void> {
        const requestBody: object = this.isEmail(username) ?
            { email: username } : { username: username };
        await this.requester.post(Constants.PASSWORD_RESET_TOKEN_PATH, {
            ...requestBody,
            siteUrlOverride: this.siteUrlOverride
        });
    }

    public async checkPasswordResetToken(token: string): Promise<void> {
        await this.requester.get(`${Constants.PASSWORD_RESET_TOKEN_PATH}/${token}`);
    }

    private isEmail(input: string): boolean {
        const re: RegExp = /\S+@\S+\.\S+/;
        return re.test(input);
    }
}