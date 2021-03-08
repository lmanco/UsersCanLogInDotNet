import { Requester } from '@/services/API/Requester';

export interface UserService {
    createUser(email: string, username: string, password: string): Promise<User>;
    getSelf(): Promise<User>;
    verifyAccount(verificationTokenId: string): Promise<void>;
    updatePassword(password: string, passwordResetTokenId: string): Promise<void>;
}

export interface User {
    id: number;
    username: string;
}

class Constants {
    public static readonly USERS_PATH: string = 'users';
    public static readonly USERS_SELF_PATH: string = `${Constants.USERS_PATH}/self`;
    public static readonly USERS_VERIFICATION_PATH: string = `${Constants.USERS_PATH}/verified`;
    public static readonly USERS_PASSWORD_UPDATE_PATH: string = `${Constants.USERS_PATH}/password`;
}

export default class RequesterUserService implements UserService {
    private requester: Requester;
    private siteUrlOverride?: string;

    public constructor(requester: Requester, siteUrlOverride?: string) {
        this.requester = requester;
        this.siteUrlOverride = siteUrlOverride;
    }

    public async createUser(email: string, username: string, password: string): Promise<User> {
        return (await this.requester.post(Constants.USERS_PATH, {
            email: email,
            username: username,
            password: password,
            siteUrlOverride: this.siteUrlOverride
        })).data;
    }

    public async getSelf(): Promise<User> {
        return (await this.requester.get(Constants.USERS_SELF_PATH)).data;
    }

    public async verifyAccount(verificationTokenId: string): Promise<void> {
        await this.requester.patch(Constants.USERS_VERIFICATION_PATH, { verificationTokenId });
    }

    public async updatePassword(password: string, passwordResetTokenId: string): Promise<void> {
        await this.requester.patch(Constants.USERS_PASSWORD_UPDATE_PATH, {
            password,
            passwordResetTokenId
        });
    }
}
