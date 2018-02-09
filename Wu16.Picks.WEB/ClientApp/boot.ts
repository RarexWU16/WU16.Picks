import './css/site.css';
import '../node_modules/bootstrap/dist/css/bootstrap.min.css';

import { Vue, VueRouter }  from './boot.vue';

const routes = [
    { path: '/', component: require('./components/home/home.vue.html') },
    { path: '/upload', component: require('./components/upload/upload.vue.html') },
    { path: '/basket', component: require('./components/basket/basket.vue.html') }
];

new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html'))
});
