import Vue from 'vue';

import VueRouter from 'vue-router';
import VueProgressiveImage from 'vue-progressive-image';
import MugenScroll from 'vue-mugen-scroll'
import { Navbar } from 'bootstrap-vue/es/components';

const customImg = require('./components/image/image.vue.html');
const spinner = require('./components/spinner/spinner.vue.html');

Vue.component("custom-img", customImg);
Vue.component("spinner", spinner);

Vue.use(MugenScroll);
Vue.use(VueProgressiveImage);
Vue.use(VueRouter);
Vue.use(Navbar);

export { Vue, VueRouter };