<script lang="ts">
import { defineComponent, h, mergeProps } from 'vue';
import type { IBaseButtonProps } from '@props';
import BaseButton from './BaseButton.vue';
import { baseBtnPropsDefault } from '~/composables/baseBtnProps';
import { useI18n } from '#imports';

export default defineComponent({
	name: 'NavigationSkipSetupButton',
	props: baseBtnPropsDefault(),
	render() {
		return h(
			BaseButton,
			{
				...mergeProps(this.$props, {
					label: useI18n().t(`general.commands.skip-setup`),
					icon: 'mdi-debug-step-over',
					iconAlign: 'right',
				} as IBaseButtonProps),
			},
			this.$slots.default,
		);
	},
});
</script>
