// swift-tools-version: 5.9
// The swift-tools-version declares the minimum version of Swift required to build this package.

import PackageDescription

let package = Package(
    name: "Rhino",
    products: [
        // Products define the executables and libraries a package produces, making them visible to other packages.
        .library(
            name: "Rhino",
            targets: ["Rhino"]),
    ],
    dependencies: [
        .package(url: "https://github.com/ash-storyboard/rhino/lib/ios/Package", from: "3.0.1"),
        .package(url: "https://github.com/ash-storyboard/ios-voice-processor/sdk/ios/Package", from: "1.1.0"),
    ],
    targets: [
        // Targets are the basic building blocks of a package, defining a module or a test suite.
        // Targets can depend on other targets in this package and products from dependencies.
        .target(
            name: "Rhino",
            dependencies: ["ios-voice-processor", "PvRhino"]),
    ]
)

