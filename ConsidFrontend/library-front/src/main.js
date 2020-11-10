import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import vuetify from './plugins/vuetify';

Vue.config.productionTip = false;

/*The following global/sessionscoped variable is just awfull.
Shuld use something else like vuex, but no time.*/
//Vue.prototype.$sortByCategory = true; //Else sort by type
window.sessionStorage.setItem('isCategorySort', true);

new Vue({
    router,
    store,
    vuetify,
    render: h => h(App)
}).$mount('#app');
