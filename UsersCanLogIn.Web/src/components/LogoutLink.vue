<template>
    <a href="#" @click="logOut" variant="primary">{{ text }}</a>
</template>

<script lang="ts">
    import { Component, Inject, Prop, Vue } from 'vue-property-decorator';
    import { LoginService } from '@/services/API/LoginService';
    import { FormState } from './enums';

    @Component
    export default class LogoutLink extends Vue {
        @Inject() readonly loginService!: LoginService;

        @Prop() readonly text!: string;

        private state: FormState = FormState.Ready;

        public async logOut(): Promise<void> {
            if (this.state !== FormState.Ready)
                return;
            await this.loginService.logout();
            this.$router.push('/login');
        } 
    }
</script>

<style scoped>
</style>