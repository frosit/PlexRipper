<script lang="ts">
import { defineComponent, h, mergeProps } from 'vue';
import type { IBaseButtonProps } from '@props';
import BaseButton from './BaseButton.vue';
import { baseBtnPropsDefault } from '~/composables/baseBtnProps';

export default defineComponent({
	name: 'IconSquareButton',
	props: baseBtnPropsDefault(),
	render() {
		return h(
			BaseButton,
			{
				...mergeProps(this.$props, {
					round: false,
					flat: true,
					outline: false,
					square: true,
				} as IBaseButtonProps),
			},
			this.$slots.default,
		);
	},
});
</script>
