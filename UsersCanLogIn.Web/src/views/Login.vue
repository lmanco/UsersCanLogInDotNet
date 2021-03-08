<template>
    <div>
        <div class="home page-header bg-dark text-light">
            <b-container>
                <b-row align-h="center">
                    <h1>Users Can Log In</h1>
                </b-row>
                <b-row align-h="center">
                    <p>A single-page app built with <a href="https://vuejs.org" target="_blank">Vue.js</a> and <a href="http://www.typescriptlang.org/" target="_blank">TypeScript</a>.</p>
                </b-row>
            </b-container>
        </div>
        <login-form :initMessages="initMessages" :initMessagesTitle="initMessagesTitle" />
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import LoginForm from '@/components/LoginForm.vue';
    import RegisterForm from '@/components/RegisterForm.vue';

    @Component({
        components: {
            LoginForm,
            RegisterForm
        }
    })
    export default class Login extends Vue {
        @Prop() readonly initMessages!: string[];
        @Prop() readonly initMessagesTitle!: string;
    }
</script>

<style scoped>
    h1 {
        margin-top: 0.5rem;
    }
</style>