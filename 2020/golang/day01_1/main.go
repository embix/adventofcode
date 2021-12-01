package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
)

// GetContent gets integer file content
func GetContent(production bool) ([]int, error) {
	var fileName string
	if production {
		fileName = "C:\\git\\adventofcode\\2020\\Inputs\\day01.personalized"
	} else {
		fileName = "C:\\git\\adventofcode\\2020\\Inputs\\day01.sample"
	}

	fd, err := os.Open(fileName)
	if err != nil {
		panic(fmt.Sprintf("failed to open %s: %v", fileName, err))
	}

	scanner := bufio.NewScanner(r)
	scanner.Split(bufio.ScanWords)
	var content []int
	for scanner.Scan() {
		i, err := strconv.Atoi(scanner.Text())
		if err != nil {
			return content, err
		}
		content = append(content, x)
	}
	return content, scanner.Err()
}
