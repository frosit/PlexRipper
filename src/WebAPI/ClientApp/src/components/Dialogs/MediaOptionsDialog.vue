<template>
	<QCardDialog
		min-width="50vw"
		max-width="50vw"
		:name="DialogType.MediaOptionsDialog"
		:loading="false"
		@opened="onOpen()">
		<template #title>
			{{
				$t('components.media-selection-dialog.title', {
					min: selectedRange.min,
					max: selectedRange.max,
				})
			}}
		</template>
		<!--	Help text	-->
		<template #default>
			<div>
				<QRow
					justify="center"
					align="center"
					class="q-pt-lg" />
				<QRow justify="between" />
			</div>
		</template>
		<!--	Close action	-->
		<template #actions="{ close }">
			<QRow justify="between">
				<QCol cols="2">
					<BaseButton
						:label="$t('general.commands.close')"
						block
						@click="close" />
				</QCol>
				<QCol cols="2">
					<BaseButton
						:label="$t('general.commands.set-selection')"
						color="positive"
						block
						@click="setSelection" />
				</QCol>
			</QRow>
		</template>
	</QCardDialog>
</template>

<script setup lang="ts">
import { get, set } from '@vueuse/core';
import { DialogType } from '@enums';
import { useMediaOverviewStore } from '#imports';

const mediaOverviewStore = useMediaOverviewStore();

const selectedRange = ref({
	min: 1,
	max: 1,
});

function adjustValue(type: string, value: number) {
	let minValue = 1;
	let maxValue = mediaOverviewStore.itemsLength;
	switch (type) {
		case 'min':
			minValue = 1;
			maxValue = get(selectedRange).max;
			break;
		case 'max':
			minValue = get(selectedRange).min;
			maxValue = mediaOverviewStore.itemsLength;
			break;
	}
	get(selectedRange)[type] = clamp(get(selectedRange)[type] + value, minValue, maxValue);
}

function setSelection() {
	mediaOverviewStore.setSelectionRange(selectedRange.value.min, selectedRange.value.max);
}

function onOpen(): void {
	set(selectedRange, {
		min: 1,
		max: mediaOverviewStore.itemsLength,
	});
}
</script>
