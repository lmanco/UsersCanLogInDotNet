<template>
    <div class="reset-password-form">
        <b-container>
            <b-row align-h="center" class="mt-5">
                <b-col cols="5">
                    <b-card class="p-3">
                        <h2 class="text-center">Reset Password</h2>
                        <b-form @submit.prevent="submitPasswordReset">
                            <b-form-group id="reset-password-form-group-password"
                                          label="New Password:"
                                          label-for="reset-password-form-input-password">
                                <b-form-input id="reset-password-form-input-password"
                                              type="password"
                                              v-model="password"
                                              required
                                              placeholder="Enter new password">
                                </b-form-input>
                            </b-form-group>
                            <b-form-group id="reset-password-form-group-confirm-password"
                                          label="Confirm New Password:"
                                          label-for="reset-password-form-input-confirm-password">
                                <b-form-input id="reset-password-form-input-confirm-password"
                                              type="password"
                                              v-model="confirmPassword"
                                              required
                                              placeholder="Enter new password again">
                                </b-form-input>
                            </b-form-group>
                            <b-button class="mt-2" type="submit" variant="primary" v-bind:disabled="setNewPasswordButtonDisabled()">Set New Password</b-button>
                            <b-container class="mt-3 p-0 text-center">
                                <div v-for="error in errors" class="error">
                                    {{ error }}
                                </div>
                            </b-container>
                        </b-form>
                    </b-card>
                </b-col>
            </b-row>
        </b-container>
    </div>
</template>

<script lang="ts">
    import { Component, Inject, Vue } from 'vue-property-decorator';
    import { UserService } from '@/services/API/UserService';
    import { FormState } from './enums';

    @Component
    export default class ResetPasswordForm extends Vue {
        @Inject() readonly userService!: UserService;

        private static readonly PASSWORD_RESET_SUCCESS_DIALOG_TITLE: string = 'Password Reset Succeeded';
        private static readonly PASSWORD_RESET_SUCCESS_DIALOG_MESSAGE: string =
            'Your new password has been set. You may now use it to log in.';

        private password: string = '';
        private confirmPassword: string = '';
        private state: FormState = FormState.Ready;
        private errors: string[] = [];

        public setNewPasswordButtonDisabled(): boolean {
            return this.state === FormState.Loading;
        }

        public async submitPasswordReset(): Promise<void> {
            if (this.setNewPasswordButtonDisabled())
                return;

            if (this.password !== this.confirmPassword) {
                this.errors = ['Password confirmation does not match password. Please re-enter it and try again.'];
                return;
            }

            this.state = FormState.Loading;

            try {
                await this.userService.updatePassword(this.password, this.$route.params.token);
                this.state = FormState.Done;
                this.errors = [];
                this.$router.replace({
                    name: 'Login', params: {
                        initMessages: [ResetPasswordForm.PASSWORD_RESET_SUCCESS_DIALOG_MESSAGE] as any,
                        initMessagesTitle: ResetPasswordForm.PASSWORD_RESET_SUCCESS_DIALOG_TITLE
                    }
                });
            }
            catch (apiResponseError) {
                this.state = FormState.Ready;
                this.errors = apiResponseError.detailMessages;
            }
        }
    }
</script>

<style scoped>
    .error {
        color: red;
    }

    button {
        width: 100%;
    }
</style>