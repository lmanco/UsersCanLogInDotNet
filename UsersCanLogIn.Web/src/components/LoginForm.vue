<template>
    <div class="login-form">
        <b-container>
            <b-row align-h="center" class="mt-5">
                <b-col cols="5">
                    <b-card class="p-3">
                        <b-form @submit.prevent="submitLogin">
                            <b-form-group id="login-form-group-username"
                                          label="Email or Username:"
                                          label-for="login-form-input-username">
                                <b-form-input id="login-form-input-username"
                                              type="text"
                                              v-model="username"
                                              required
                                              placeholder="Enter email or username">
                                </b-form-input>
                            </b-form-group>
                            <b-form-group id="form-group-password"
                                          label="Password:"
                                          label-for="login-form-input-password">
                                <b-form-input id="login-form-input-password"
                                              type="password"
                                              v-model="password"
                                              required
                                              placeholder="Enter password">
                                </b-form-input>
                            </b-form-group>
                            <b-button class="mt-2" type="submit" variant="primary" v-bind:disabled="loginButtonDisabled()">Log In</b-button>
                            <b-container class="mt-3 p-0 text-center">
                                <div v-for="error in errors" class="error">
                                    {{ error }}
                                </div>
                                <div v-if="showResendVerificationEmail">
                                    <hr/>
                                    <resend-verification-email :email="username" />
                                    <hr/>
                                </div>
                            </b-container>
                            <b-container class="mt-3 p-0 text-center">
                                <b-link href="#" v-b-modal.forgot-password-modal>Forgot Password?</b-link>
                            </b-container>
                            <hr/>
                            <b-container class="text-center">
                                New User?
                            </b-container>
                            <b-button class="mt-2" variant="success" v-bind:disabled="loginButtonDisabled()" v-b-modal.register-modal>
                                Create Account
                            </b-button>
                        </b-form>
                    </b-card>
                </b-col>
            </b-row>
        </b-container>
        <b-modal id="register-modal" ref="register-modal" title="Create New Account" :hide-footer="true">
            <register-form :complete="hideRegisterModal" />
        </b-modal>
        <b-modal id="forgot-password-modal" ref="forgot-password-modal" title="Reset Password"
                 :hide-footer="true" @hidden="resetForgotPasswordModal">
            <b-container>
                <b-form ref="forgot-password-form" @submit.prevent="sendPasswordResetEmail">
                    <b-form-group id="forgot-password-form-input-username"
                                  label=" Please enter your email or username and click the Send Password Reset Email button below.
                                   An email with a link to reset your password will be sent to you."
                                  label-for="register-form-input-username">
                        <b-form-input id="register-form-input-username"
                                      type="text"
                                      v-model="resetPasswordUsername"
                                      required
                                      placeholder="Enter email or username">
                        </b-form-input>
                    </b-form-group>
                    <b-button class="mt-2" type="submit" variant="primary" v-bind:disabled="forgotPasswordFormButtonDisabled()">
                        Send Password Reset Email
                    </b-button>
                </b-form>
                <b-container class="mt-2 text-center">
                    <div v-if="passwordResetEmailDone()" :class="{ success: true, 'fade-success': fadeResetPasswordSuccess }">
                        Password reset email sent.
                    </div>
                    <div v-for="error in resetPasswordErrors" class="error">
                        {{ error }}
                    </div>
                </b-container>
            </b-container>
        </b-modal>
        <b-modal id="init-messages-modal" ref="init-messages-modal" :title="initMessagesTitle" ok-only>
            <b-container>
                <div v-for="initMessage in initMessages">
                    {{ initMessage }}
                </div>
            </b-container>
        </b-modal>
    </div>
</template>

<script lang="ts">
    import { Component, Inject, Prop, Vue } from 'vue-property-decorator';
    import { LoginService } from '@/services/API/LoginService';
    import { FormState } from './enums';
    import RegisterForm from '@/components/RegisterForm.vue';
    import ResendVerificationEmail from '@/components/ResendVerificationEmail.vue';
    import { APIError } from '../services/API/types/APIResponses';

    @Component({
        components: {
            RegisterForm,
            ResendVerificationEmail
        }
    })
    export default class LoginForm extends Vue {
        @Inject() readonly loginService!: LoginService;

        @Prop() readonly initMessagesTitle!: string;
        @Prop() readonly initMessages!: string[];

        private static readonly NOT_VERIFIED_ERROR_TITLE: string = 'Account Not Verified';
        private static readonly NOT_VERIFIED_APPEND: string = ' Please click on the activation link in your account confirmation email.';

        private username: string = '';
        private password: string = '';
        private state: FormState = FormState.Ready;
        private showResendVerificationEmail: boolean = false;
        private errors: string[] = [];

        private resetPasswordUsername: string = '';
        private forgotPasswordFormState: FormState = FormState.Ready;
        private fadeResetPasswordSuccess: boolean = false;
        private resetPasswordErrors: string[] = [];

        public loginButtonDisabled(): boolean {
            return this.state !== FormState.Ready;
        }

        public async submitLogin(): Promise<void> {
            if (this.loginButtonDisabled())
                return;

            this.state = FormState.Loading;

            try {
                await this.loginService.login(this.username, this.password);
                this.$router.push('/');
                this.errors = [];
                this.showResendVerificationEmail = false;
            }
            catch (apiResponseError) {
                this.state = FormState.Ready;
                apiResponseError.RewriteErrorDetail(LoginForm.NOT_VERIFIED_ERROR_TITLE, LoginForm.NOT_VERIFIED_APPEND, true);
                this.errors = apiResponseError.detailMessages;
                const errorTitleLower: string = LoginForm.NOT_VERIFIED_ERROR_TITLE.toLowerCase();
                this.showResendVerificationEmail = apiResponseError.apiErrors
                    .some((error: APIError) => error.title.toLowerCase() === errorTitleLower);
            }
        }

        public hideRegisterModal(): void {
            (this.$refs['register-modal'] as any).hide()
        }

        public forgotPasswordFormButtonDisabled(): boolean {
            return this.forgotPasswordFormState !== FormState.Ready;
        }

        public async sendPasswordResetEmail(): Promise<void> {
            if (this.forgotPasswordFormState !== FormState.Ready)
                return;

            this.forgotPasswordFormState = FormState.Loading;

            try {
                await this.loginService.sendPasswordResetEmail(this.resetPasswordUsername);
                this.resetPasswordErrors = [];
                this.forgotPasswordFormState = FormState.Done;
                this.fadeResetPasswordSuccess = false;
                this.resetForgotPasswordForm();
            }
            catch (apiResponseError) {
                this.forgotPasswordFormState = FormState.Ready;
                this.resetPasswordErrors = apiResponseError.detailMessages;
            }
        }

        public resetForgotPasswordForm(): void {
            setTimeout(() => {
                this.fadeResetPasswordSuccess = true;
                setTimeout(() => {
                    this.forgotPasswordFormState = FormState.Ready;
                }, 1000);
            }, 1000);
        }

        public passwordResetEmailDone(): boolean {
            return this.forgotPasswordFormState === FormState.Done;
        }

        public resetForgotPasswordModal(): void {
            this.resetPasswordUsername = '';
            this.forgotPasswordFormState = FormState.Ready;
            this.fadeResetPasswordSuccess = false;
            this.resetPasswordErrors = [];
        }

        public mounted(): void {
            if (this.initMessages && this.initMessages.length)
                (this.$refs['init-messages-modal'] as any).show();
        }
    }
</script>

<style scoped>
    .login-form {
        display: flex;
        flex-direction: column;
        align-items: center;
        margin-top: 0.5rem;
    }

    .error {
        color: red;
    }

    button {
        width: 100%;
    }

    .success {
        color :#28a745;
        transition: opacity 1s;
    }

    .fade-success {
        opacity: 0;
    }
</style>