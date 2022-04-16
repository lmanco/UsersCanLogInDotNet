import Vue from 'vue';
import App from './App.vue';
import router from './router';
import BootstrapVue from 'bootstrap-vue';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';
import VueMq from 'vue-mq';

Vue.use(BootstrapVue);

Vue.use(VueMq, {
    breakpoints: {
        mobile: 576,
        tablet: 900,
        notebook: 1180,
        laptop: 1250,
        desktop: Infinity
    }
});

Vue.mixin({
    data: function () {
        return {
            get mqOrdinals() {
                return {
                    mobile: 0,
                    tablet: 1,
                    notebook: 2,
                    laptop: 3,
                    desktop: 4
                }
            }
        }
    },
    computed: {
        isMobile: function () {
            return this.mqOrdinal === (this.mqOrdinals as any).mobile;
        },
        isTablet: function () {
            return this.mqOrdinal === (this.mqOrdinals as any).tablet;
        },
        isNotebook: function () {
            return this.mqOrdinal === (this.mqOrdinals as any).notebook;
        },
        isLaptop: function () {
            return this.mqOrdinal === (this.mqOrdinals as any).laptop;
        },
        isDesktop: function () {
            return this.mqOrdinal === (this.mqOrdinals as any).desktop;
        },
        mqOrdinal: function () {
            return (this.mqOrdinals as any)[this.$mq as string];
        }
    }
})

Vue.config.productionTip = false;

new Vue({
    router,
    render: h => h(App)
}).$mount('#app');
