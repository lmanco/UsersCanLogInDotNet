<template>
    <b-container class="text-center mt-5">
        <h1>Page Not Found</h1>
        <p>Unfortunately, the requested page does not exist. <a href="/">Click here</a> to return to the home page.</p>
    </b-container>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';

    @Component
    export default class NotFound extends Vue {}
</script>

<style scoped>
</style>