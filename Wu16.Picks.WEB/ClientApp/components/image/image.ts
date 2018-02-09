import Vue from 'vue';
import { Component, Prop, Inject } from 'vue-property-decorator';
import { Image } from '../../interfaces';
import { toggleBasket } from '../../Services/http';
import { url } from '../../Services/general';

@Component
export default class ImageComponent extends Vue {
    @Prop({ "default": "image" })
    public image: Image;

    public padding(ratio: number): string {
        return `padding-bottom:${ratio * 100}%`;
    }

    public toggleBasket() {
        toggleBasket(this.image.id, result => this.image.isInBasket = result);
    }

    public url = url;
}
