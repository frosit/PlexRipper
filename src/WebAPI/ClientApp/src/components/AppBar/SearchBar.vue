<template>
	<q-select
		v-model="model"
		:option-label="'title' satisfies keyof PlexMediaSlimDTO"
		use-input
		fill-input
		hide-selected
		:options="options"
		behavior="menu"
		@filter="filterFn"
		@filter-abort="abortFilterFn"
		@input-value="query = $event">
		<template #option="{ opt, itemProps } : {opt: PlexMediaSlimDTO, itemProps: any}">
			<q-item v-bind="itemProps">
				<q-item-section avatar>
					<q-img
						loading="eager"
						fit="fill"
						no-spinner
						ratio="2/3"
						:width="thumbWidth + 'px'"
						:height="thumbHeight + 'px'"
						:src="`&width=${thumbWidth}&height=${thumbHeight}`" /> <!-- Needs correct url -->
				</q-item-section>
				<q-item-section>
					<q-item-label>{{ opt.title }}</q-item-label>
					<q-item-label caption>
						{{ opt.title }}
					</q-item-label>
				</q-item-section>
			</q-item>
		</template>
		<!-- No Option -->
		<template #no-option>
			<q-item v-if="query.length > 2">
				<q-item-section class="text-grey">
					<QText :value="$t('components.search-bar.no-results', { query })" />
				</q-item-section>
			</q-item>
		</template>
	</q-select>
</template>

<script setup lang="ts">
import { set } from '@vueuse/core';
import { plexMediaApi } from '@api';
import type { PlexMediaSlimDTO } from '@dto';

const model = ref<PlexMediaSlimDTO | null>(null);
const query = ref<string>('');

const thumbWidth = ref(80);
const thumbHeight = ref(120);

const options = ref([]);

function filterFn(val: string, update: (callback: () => void) => void, abort: () => void) {
	if (val === '') {
		update(() => {
			set(options, []);
		});
		return;
	}

	useSubscription(plexMediaApi.searchPlexMediaEndpoint({
		query: val,
	})
		.subscribe({
			next(data) {
				update(() =>
					set(options, data.value ?? []),
				);
			},
			error() {
				abort();
			},
		}));
}

function abortFilterFn() {
	// console.log('delayed filter aborted')
}
</script>

<style lang="scss">

</style>
