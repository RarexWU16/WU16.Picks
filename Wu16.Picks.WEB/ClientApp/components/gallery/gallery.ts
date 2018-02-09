import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import { Image } from '../../interfaces';

@Component
export default class GalleryComponent extends Vue {
    @Prop({ "default": "images" })
    public images: Image[];

    get rows(): Image[][] {
        const rows: Image[][] = [];

        const images: Image[] = this.images.slice();
        while (images.length > 0) {
            rows.push(images.splice(0, 3));
        }

        return rows;
    };
}
