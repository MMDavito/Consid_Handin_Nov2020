import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import vuetify from './plugins/vuetify';

Vue.config.productionTip = false;

new Vue({
    /*The following global/sessionscoped variable is just awfull.
    Shuld use something else like vuex, but no time.*/
    data: {
        sortByCategory: true //Else sort by type
    },
    router,
    store,
    vuetify,
    render: h => h(App)
}).$mount('#app');
