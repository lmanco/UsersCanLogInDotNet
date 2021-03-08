<template>
    <div id="app">
        <router-view/>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Provide } from 'vue-property-decorator';
    import RequesterLoginService, { LoginService } from "./services/API/LoginService";
    import { FetchRequester, Requester } from './services/API/Requester';
    import RequesterUserService, { UserService } from './services/API/UserService';

    interface Services {
        requester: Requester;
        loginService: LoginService;
        userService: UserService;
    }

    const requester: Requester = new FetchRequester();
    const siteUrlOverride: string | undefined = process.env.NODE_ENV === 'development' ?
        `${window.location.protocol}//${window.location.host}` : undefined;
    const services: Services = {
        requester,
        loginService: new RequesterLoginService(requester, siteUrlOverride),
        userService: new RequesterUserService(requester, siteUrlOverride)
    };

    @Component
    export default class App extends Vue {
        @Provide() loginService = services.loginService;
        @Provide() userService = services.userService;
    }
</script>

<style>
    #app {
        font-family: Avenir, Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        color: #2c3e50;
    }

    #nav {
        padding: 30px;
    }

    #nav a {
        font-weight: bold;
        color: #2c3e50;
    }

    #nav a.router-link-exact-active {
        color: #42b983;
    }

    body {
        background-color: #EEF1F4 !important;
    }
</style>
