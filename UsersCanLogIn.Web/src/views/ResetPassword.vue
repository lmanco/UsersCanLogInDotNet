<template>
    <reset-password-form />
</template>

<script lang="ts">
    import { Component, Inject, Vue } from 'vue-property-decorator';
    import ResetPasswordForm from '@/components/ResetPasswordForm.vue';
    import { LoginService } from '../services/API/LoginService';

    @Component({
        components: {
            ResetPasswordForm
        }
    })
    export default class ResetPassword extends Vue {
        @Inject() readonly loginService!: LoginService;

        private static readonly PASSWORD_RESET_FAILED_DIALOG_TITLE: string = 'Password Reset Failed';

        public async mounted(): Promise<void> {
            try {
                await this.loginService.checkPasswordResetToken(this.$route.params.token);
            }
            catch (apiResponseError) {
                this.$router.replace({
                    name: 'Login', params: {
                        initMessages: apiResponseError.detailMessages as any,
                        initMessagesTitle: ResetPassword.PASSWORD_RESET_FAILED_DIALOG_TITLE
                    }
                });
            }
        }
    }
</script>

<style scoped>
</style>