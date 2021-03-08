<template>
</template>

<script lang="ts">
    import { Component, Inject, Vue } from 'vue-property-decorator';
    import { UserService } from '@/services/API/UserService';

    @Component
    export default class ActivateAccount extends Vue {
        @Inject() readonly userService!: UserService;

        private static readonly ACTIVATION_FAILED_DIALOG_TITLE: string = 'Account Activation Failed';

        public async mounted(): Promise<void> {
            try {
                await this.userService.verifyAccount(this.$route.params.token);
                this.$router.replace({ name: 'Home', params: { showActivationSuccessMessage: true as any } });
            }
            catch (apiResponseError) {
                this.$router.replace({
                    name: 'Login', params: {
                        initMessages: apiResponseError.detailMessages as any,
                        initMessagesTitle: ActivateAccount.ACTIVATION_FAILED_DIALOG_TITLE
                    }
                });
            }
        }
    }
</script>

<style scoped>
</style>