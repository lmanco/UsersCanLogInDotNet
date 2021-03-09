import { Component, Inject, Vue } from 'vue-property-decorator';
import { User, UserService } from '@/services/API/UserService';

@Component
export default class AuthenticatedView extends Vue {
    @Inject() readonly userService!: UserService;

    protected authenticatedUser!: User;
    protected authenticated: boolean = false;

    public async mounted(): Promise<void> {
        try {
            this.authenticatedUser = await this.userService.getSelf();
            this.authenticated = true;
        }
        catch (apiResponseError) {
            this.$router.replace('/login');
        }
    }
}