<template>
    <div v-if="authenticated">
        <b-alert v-if="showActivationSuccessMessage" id="account-activated-alert" variant="success" dismissible show>
            Email confirmed. Your account is now activated!
        </b-alert>
        <logout-button />
        <b-container class="text-center mt-5">
            <h1>Welcome, {{ authenticatedUser.username }}!</h1>
            <p>You are now logged in! <logout-link text="Click here">Click here</logout-link> to log out.</p>
        </b-container>
    </div>
</template>

<script lang="ts">
    import { Component, Prop } from 'vue-property-decorator';
    import AuthenticatedView from './AuthenticatedView';
    import LogoutLink from '@/components/LogoutLink.vue';

    @Component({
        components: {
            LogoutLink
        }
    })
    export default class Home extends AuthenticatedView {
        @Prop() showActivationSuccessMessage!: boolean;

    }
</script>

<style scoped>
</style>
