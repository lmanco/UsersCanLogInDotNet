<template>
    <div>
        <div v-if="resendLinkDisabled()" class="text-center">
            Didn't get the confirmation email? Click here to resend it.
        </div>
        <div v-else class="text-center">
            Didn't get the confirmation email?
            <a href="#" :class="{ 'disabled-link': resendLinkDisabled() }" @click="sendEmail"> Click here</a>
            to resend it.
        </div>
        <b-container class="mt-2 text-center">
            <div v-if="done()" :class="{ success: true, 'fade-success': fadeSuccess }">
                Confirmation email resent.
            </div>
            <div v-for="error in errors" class="error">
                {{ error }}
            </div>
        </b-container>
    </div>
</template>

<script lang="ts">
    import { Component, Inject, Prop, Vue } from 'vue-property-decorator';
    import { FormState } from './enums';
    import { LoginService } from '@/services/API/LoginService';

    @Component
    export default class ResendVerificationEmail extends Vue {
        @Inject() readonly loginService!: LoginService;

        @Prop() readonly email!: string;

        private state: FormState = FormState.Ready;
        private fadeSuccess: boolean = false;
        private errors: string[] = [];

        public resendLinkDisabled(): boolean {
            return this.state !== FormState.Ready;
        }

        public async sendEmail() {
            if (this.state !== FormState.Ready)
                return;

            this.state = FormState.Loading;

            try {
                await this.loginService.resendVerificationEmail(this.email);
                this.errors = [];
                this.state = FormState.Done;
                this.fadeSuccess = false;
                this.reset();
            }
            catch (apiResponseError) {
                this.state = FormState.Ready;
                this.errors = apiResponseError.detailMessages;
            }
        }

        public reset(): void {
            setTimeout(() => {
                this.fadeSuccess = true;
                setTimeout(() => {
                    this.state = FormState.Ready;
                }, 1000);
            }, 1000);
        }

        public done(): boolean {
            return this.state === FormState.Done;
        }
    }
</script>

<style scoped>
    .error {
        color: red;
    }

    .success {
        color :#28a745;
        transition: opacity 1s;
    }

    .fade-success {
        opacity: 0;
    }
</style>