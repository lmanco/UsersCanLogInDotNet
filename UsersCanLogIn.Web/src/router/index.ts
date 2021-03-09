import Vue from 'vue'
import VueRouter from 'vue-router'
import Login from '../views/Login.vue'
import Home from '../views/Home.vue'
import ActivateAccount from '../views/ActivateAccount.vue'
import ResetPassword from '../views/ResetPassword.vue'
import NotFound from '../views/NotFound.vue'

Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'Home',
        component: Home,
        props: (route: any) => route.params
    },
    {
        path: '/login',
        name: 'Login',
        component: Login,
        props: (route: any) => route.params
    },
    {
        path: '/activate/:token',
        name: 'ActivateAccount',
        component: ActivateAccount
    },
    {
        path: '/reset-password/:token',
        name: 'ResetPassword',
        component: ResetPassword,
        props: (route: any) => route.params
    },
    {
        path: '*',
        name: 'Not Found',
        component: NotFound
    }
]

const router = new VueRouter({
    mode: 'history',
    routes
})

export default router
