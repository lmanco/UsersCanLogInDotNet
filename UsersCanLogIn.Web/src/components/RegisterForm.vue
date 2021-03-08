<template>
    <div class="register-form">
        <b-container>
            <b-container v-if="!done()">
                <b-form  @submit.prevent="submitRegister">
                    <b-form-group id="register-form-group-email"
                                  label="Email:"
                                  label-for="register-form-input-email">
                        <b-form-input id="register-form-input-email"
                                      type="text"
                                      v-model="email"
                                      required
                                      placeholder="Enter email">
                        </b-form-input>
                    </b-form-group>
                    <b-form-group id="register-form-group-username"
                                  label="Username:"
                                  label-for="register-form-input-username">
                        <b-form-input id="register-form-input-username"
                                      type="text"
                                      v-model="username"
                                      required
                                      placeholder="Enter username">
                        </b-form-input>
                    </b-form-group>
                    <b-form-group id="register-form-group-password"
                                  label="Password:"
                                  label-for="register-form-input-password">
                        <b-form-input id="register-form-input-password"
                                      type="password"
                                      v-model="password"
                                      required
                                      placeholder="Enter password">
                        </b-form-input>
                    </b-form-group>
                    <b-form-group id="register-form-group-confirm-password"
                                  label="Confirm Password:"
                                  label-for="register-form-input-confirm-password">
                        <b-form-input id="register-form-input-confirm-password"
                                      type="password"
                                      v-model="confirmPassword"
                                      required
                                      placeholder="Enter password again">
                        </b-form-input>
                    </b-form-group>
                    <b-button class="mt-2" type="submit" variant="success" v-bind:disabled="registerButtonDisabled()">Register</b-button>
                    <b-container class="mt-3 text-center">
                        <div v-for="error in errors" class="error">
                            {{ error }}
                        </div>
                    </b-container>
                </b-form>
            </b-container>
            <b-container v-else>
                <div class="registered-message text-center">
                    A confirmation email has been sent to {{ email }}.
                    Please verify your account by clicking the activation link in the confirmation email.
                </div>
                <b-row align-h="center" class="mt-2">
                    <b-col cols="3">
                        <b-button variant="primary" @click="complete()">OK</b-button>
                    </b-col>
                </b-row>
                <hr/>
                <resend-verification-email :email="email" />
            </b-container>
        </b-container>
    </div>
</template>

<script lang="ts">
    import { Component, Inject, Prop, Vue } from 'vue-property-decorator';
    import { UserService } from '@/services/API/UserService';
    import { FormState } from './enums';
    import ResendVerificationEmail from '@/components/ResendVerificationEmail.vue';

    @Component({
        components: {
            ResendVerificationEmail
        }
    })
    export default class RegisterForm extends Vue {
        @Inject() readonly userService!: UserService;

        @Prop() readonly complete!: () => void;

        private email: string = '';
        private username: string = '';
        private password: string = '';
        private confirmPassword: string = '';
        private state: FormState = FormState.Ready;
        private errors: string[] = [];

        public registerButtonDisabled(): boolean {
            return this.state === FormState.Loading;
        }

        public async submitRegister(): Promise<void> {
            if (this.registerButtonDisabled())
                return;

            if (this.password !== this.confirmPassword) {
                this.errors = ['Password confirmation does not match password. Please re-enter it and try again.'];
                return;
            }

            this.state = FormState.Loading;

            try {
                await this.userService.createUser(this.email, this.username, this.password);
                this.state = FormState.Done;
                this.errors = [];
            }
            catch (apiResponseError) {
                this.state = FormState.Ready;
                this.errors = apiResponseError.detailMessages;
            }
        }

        public done(): boolean {
            return this.state === FormState.Done;
        }
    }
</script>

<style scoped>
    .register-form {
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    .register-form-input label {
        margin-bottom: 0;
    }

    button {
        width: 100%;
    }

    .error {
        color: red;
    }

    .registered-message {
        color: #007bff;
    }
</style>
