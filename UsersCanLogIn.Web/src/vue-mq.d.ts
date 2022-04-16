declare module 'vue-mq' {
    import { PluginObject } from 'vue';

    interface VueMq extends PluginObject<any> {
        VueMq: VueMq;
    }

    const VueMq: VueMq;
    export default VueMq;
}